import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { PresenceService } from './presence.service';


// Ce décorateur indique que la classe AccountService est un
// service Injectable et qu'il est de type Singelton
@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;

  private currentUserSource = new ReplaySubject<User>(1);

  // Selon la convention, on ajoute $ devant tout Observable
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient, private presenceService: PresenceService) { }

  login(model: any) {
    return this.http.post(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
          this.setCurrentUser(user);
          // Quand l'utilisateur se connecte, on crée le hub de connection
          this.presenceService.createHubConnection(user); 
        }
      })
    );
  }

  register(model: any) {
    return this.http.post(this.baseUrl + 'account/register', model).pipe(
      map((user: User) => {
        if (user) {
          this.setCurrentUser(user);
          // Quand l'utilisateur se connecte, on crée le hub de connection
          this.presenceService.createHubConnection(user); 
        }
      })
    );
  }

  setCurrentUser(user: User) {
    if (user !== null) {
      user.roles = [];
      const roles = this.getDecodedToken(user.token).role;
      // Comme roles peut être soit:
      //   - un array quand user a plusieurs roles
      //   - un string quand il n'a qu'un seul role
      // donc, on le gère différement dans les deux cas.
      Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
    }

    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }

  logout() {
    localStorage.removeItem('user');
    this.setCurrentUser(null);
  
    // Quand l'utilisateur se déconnecte, on arrête le hub de connection
    this.presenceService.stopHubConnection();
  }

  getDecodedToken(token) {
    return JSON.parse(atob(token.split('.')[1]));
  }
}

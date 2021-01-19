import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { User } from '../_models/user';
import { UserParams } from '../_models/userParams';
import { PaginatedResult } from '../_modules/pagination';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  // Ce tableau serait pour stocker les members afin de faire un StoreState,
  // pour éviter de faire des du trafic réseau tout le temps.
  members: Member[] = [];

  // MmemberCache est un dictionnaire pour gérer le cache des members-list
  // basé sur le stockage "en key" des paramUser sous forme de chaine de carac
  // et "en value" de la response renvoyé par l'Api
  memberCache = new Map();

  user: User;
  userParams: UserParams;

  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
      this.userParams = new UserParams(user);
    });
  }

  getUserParams() {
    return this.userParams;
  }

  setUserParams(params: UserParams) {
    this.userParams = params;
  }

  resetUserParams() {
    this.userParams = new UserParams(this.user);
    return this.userParams;
  }

  getMembers(userParams: UserParams) {
    // Avant la pagination
    /* if (this.members.length > 0) { return of(this.members); } */
    /* L'inconveniant "members" contient tous les users, ce qui risque d'être lourd au niveau du chargement
    return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
      map(result => {
        this.members = result;
        return this.members;
      })
    );
    */

    // Pour visualiser les params reçus
    // console.log(Object.values(userParams).join('-'));
    const response = this.memberCache.get(Object.values(userParams).join('-'));
    if (response) {
      return of(response);
    }

    let params = this.getPaginationHeaders(userParams.pageNumber, userParams.pageSize);
    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    return this.getPaginationResult<Member[]>(this.baseUrl + 'users', params)
    .pipe(
      map(result => {
          this.memberCache.set(Object.values(userParams).join('-'), result);
          return result;
      })
    );
  }

  private getPaginationResult<T>(url, params) {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();

    return this.http.get<T>(url, { observe: 'response', params }).pipe(
      /* avec Pagination */
      map(response => {
        paginatedResult.result = response.body;
        if (response.headers.get('Pagination') !== null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResult;
      })
    );
  }

  private getPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams();
    params = params.append('pageNumber', pageNumber.toString());
    params = params.append('pageSize', pageSize.toString());

    return params;
  }

  getMember(username: string) {
    // console.log(this.memberCache);

    // Reduce, permet de transferer le contenu du dictionnaire dans un tableau
    const memberToReturn = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.result), [])
      .find((member: Member) => member.username === username);
    // console.log(memberToReturn);
    if (memberToReturn) {
      return of(memberToReturn);
    }

    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    );
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }
}

import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm: NgForm;
  /* 
  Ce listener permet de détecter si l'utilisateur tente d'aller sur une url différente 
  via la barre d'adresse du navigateur. Dans ce cas là, il est averti par une popup.
  */
  @HostListener("window:beforeunload", ["$event"]) 
  unloadNotification($event: any) {
    if(this.editForm.dirty) {
      $event.retunValue = true;
    }
  }

  member: Member;
  user: User;

  constructor(
    private accountService: AccountService, 
    private memberService: MembersService,
    private toastr: ToastrService) { 
      this.accountService.currentUser$.pipe(take(1)).subscribe( usr => this.user = usr);
  }

  ngOnInit(): void {
    this.getMember();
  }

  getMember() {
    this.memberService.getMember(this.user.username).subscribe(result => this.member = result);
  }

  updateMember() {
    this.memberService.updateMember(this.member).subscribe(() => {
      this.toastr.success("Profile updated succeffuly");
      this.editForm.reset(this.member);
    });
  }
}
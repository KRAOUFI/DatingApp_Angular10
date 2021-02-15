import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Message } from 'src/app/_models/message';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm: NgForm;
  /*@Input() messages: Message[] = [];*/
  @Input() username: string;
  messageContent: string;

  constructor(public messageService: MessageService) { }

  ngOnInit(): void { }

  sendMessage() {
    /* La methode avec l'Observable
    this.messageService.sendMessage(this.username, this.messageContent).subscribe(response => {
      this.messages.push(response);
      this.messageForm.reset();
    });
    */
    
    // Puisque la méthode ne retourne plus rien, alors il suffit juste de détecter qu'elle 
    // terminé le traitement pour poursuivre les autres actions
    this.messageService.sendMessage(this.username, this.messageContent).then(() => {
      this.messageForm.reset();
    });

  }
}

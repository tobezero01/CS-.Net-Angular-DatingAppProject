import { Component, inject, OnInit } from '@angular/core';
import { MessageService } from '../_services/message.service';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { TimeagoModule } from 'ngx-timeago';
import { Message } from '../_models/message';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { PaginationModule } from 'ngx-bootstrap/pagination';

@Component({
  selector: 'app-messages',
  standalone: true,
  imports: [FormsModule, RouterLink, TimeagoModule, ButtonsModule, PaginationModule],
  templateUrl: './messages.component.html',
  styleUrl: './messages.component.css'
})
export class MessagesComponent implements OnInit{
  [x: string]: any;

  messagesService = inject(MessageService);
  container = "Inbox";
  pageNumber = 1;
  pageSize = 5;
  isOutbox = this.container === "Outbox";

  ngOnInit(): void {
    throw new Error('Method not implemented.');
  }

  loadMessages() {
    this.messagesService.getMessages(this.pageNumber, this.pageSize, this.container);
  }

  deleteMessage(id : number) {
    this.messagesService.deleteMessage(id).subscribe({
      next : _ => {
        this.messagesService.paginatedResult.update(prev => {
          if ( prev && prev.items) {
            prev.items.splice(prev.items.findIndex(m => m.id == id), 1);
            return prev;
          }
          return prev;
        })
      }
    });
  }

  getRoute(message : Message) {
    if(this.container == "Outbox" ) return `/members/${message.recipientUsername}`;
    else return `/members/${message.senderUsername}`;
  }

  pageChanged(event : any) {
    if( this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.loadMessages();
    }
  }
}

import { Message } from './../../_models/message';
import { Component, inject, input, OnInit, output, ViewChild } from '@angular/core';
import { MessageService } from '../../_services/message.service';
import { FormsModule, NgForm } from '@angular/forms';
import { TimeagoModule } from 'ngx-timeago';
import { NgFor } from '@angular/common';

@Component({
  selector: 'app-member-messages',
  standalone: true,
  imports: [ FormsModule, TimeagoModule, FormsModule],
  templateUrl: './member-messages.component.html',
  styleUrl: './member-messages.component.css'
})
export class MemberMessagesComponent {
  @ViewChild('messageForm') messageForm? : NgForm;
  private messagesService = inject(MessageService);
  username = input.required<string>();
  messages = input.required<Message[]>();
  messageContent = '';
  updateMessages = output<Message>();

  sendMessage() {
    this.messagesService.sendMessage(this.username(), this.messageContent).subscribe({
      next : message => {
        this.updateMessages.emit(message);
        this.messageForm?.reset();
      }
    })
  }


}

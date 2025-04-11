import { MessageService } from './../../_services/message.service';
import { TimeagoModule, TimeagoPipe } from 'ngx-timeago';
import { map } from 'rxjs';
import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { MembersService } from '../../_services/members.service';
import { ActivatedRoute } from '@angular/router';
import { Member } from '../../_models/member';
import { TabDirective, TabsetComponent, TabsModule } from 'ngx-bootstrap/tabs';
import { GalleryItem, GalleryModule, ImageItem} from 'ng-gallery';
import { DatePipe } from '@angular/common';
import { MemberMessagesComponent } from "../member-messages/member-messages.component";
import { Message } from '../../_models/message';
import { PresenceService } from '../../_services/presence.service';
@Component({
  selector: 'app-member-detail',
  standalone: true,
  imports: [TabsModule, GalleryModule, TimeagoModule, DatePipe, MemberMessagesComponent],
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.css'
})
export class MemberDetailComponent implements OnInit{
  @ViewChild('memberTabs', {static: true}) memberTab?: TabsetComponent;
  memberService = inject(MembersService);
  route = inject(ActivatedRoute);
  messagesService = inject(MessageService);
  presenceService = inject(PresenceService);
  member: Member = {} as Member;
  images : GalleryItem[] = [];
  activeTab? : TabDirective;
  messages: Message[] = [];

  ngOnInit(): void {
    this.route.data.subscribe({
      next: data => {
        this.member = data['member'];
        this.member && this.member.photos.map(p => {
          this.images.push(new ImageItem({src: p.url, thumb: p.url}))
        })
      }
    })

    this.route.queryParams.subscribe({
      next : params => {
        params['tab'] && this.selectTab(params['tab'])
      }
    })
  }

  onUpdateMessages(event : Message) {
    this.messages.push(event);
  }

  selectTab(heading : string) {
    if(this.memberTab) {
      const messageTab = this.memberTab.tabs.find(x => x.heading === heading) ;
      if ( messageTab) messageTab.active = true;
    }
  }

  onTabActivated(data : TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages' && this.messages.length === 0 && this.member) {
      this.messagesService.getMessageThread(this.member.username).subscribe({
        next : messages => this.messages = messages
      })
    }
  }


}

import { Member } from './../../_models/member';
import { Component , input, ViewEncapsulation} from '@angular/core';

@Component({
  selector: 'app-member-card',
  standalone: true,
  imports: [],
  templateUrl: './member-card.component.html',
  styleUrl: './member-card.component.css',
  encapsulation : ViewEncapsulation.None
})
export class MemberCardComponent {
  member = input.required<Member>();
}

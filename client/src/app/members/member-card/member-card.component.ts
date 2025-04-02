import { RouterLink } from '@angular/router';
import { Member } from './../../_models/member';
import { Component , computed, inject, input, ViewEncapsulation} from '@angular/core';
import { LikesService } from '../../_services/like.service';

@Component({
  selector: 'app-member-card',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './member-card.component.html',
  styleUrl: './member-card.component.css',
  encapsulation : ViewEncapsulation.None
})
export class MemberCardComponent {
  likeService = inject(LikesService);
  member = input.required<Member>();
  hasLiked = computed(() => this.likeService.likeIds().includes(this.member().id))

  toggleLike() {
    this.likeService.toggleLike(this.member().id).subscribe({
      next : () => {
        if ( this.hasLiked() ) {
          this.likeService.likeIds.update(ids => ids.filter(x => x !== this.member().id))
        }
        else {
          this.likeService.likeIds.update( ids => [...ids, this.member().id])
        }
      }
    })
  }
}

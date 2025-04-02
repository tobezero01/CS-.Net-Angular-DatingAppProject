import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { User } from '../_models/User';
import { map } from 'rxjs';
import { LikesService } from './like.service';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  private likesService = inject(LikesService);
  currentUser = signal<User | null>(null);

  login(model : any) {
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map(user => {
        if(user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  register(model : any) {
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map(user => {
        if(user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUser.set(user);

        }
        return user;
      })
    );
  }

  setCurrentUser(user : User) {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUser.set(user);
    this.likesService.getLikeIds();
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUser.set(null);
  }
}

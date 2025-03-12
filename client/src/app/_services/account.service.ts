import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { User } from '../_models/User';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'  // Service này có thể sử dụng trong toàn bộ ứng dụng
})
export class AccountService {
  private http = inject(HttpClient);  // Inject HttpClient để gọi API
  baseUrl = 'http://localhost:5001/api/';  // Địa chỉ API backend
  currentUser = signal<User | null>(null); // Quản lý trạng thái đăng nhập (null = chưa đăng nhập)

  // 📌 Hàm đăng nhập
  login(model : any) {
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map(user => {
        if(user) {
          localStorage.setItem('user', JSON.stringify(user)); // Lưu token vào LocalStorage
          this.currentUser.set(user); // Cập nhật trạng thái đăng nhập
        }
      })
    );
  }

  register(model : any) {
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map(user => {
        if(user) {
          localStorage.setItem('user', JSON.stringify(user)); // Lưu token vào LocalStorage
          this.currentUser.set(user); // Cập nhật trạng thái đăng nhập
        }
        return user;
      })
    );
  }

  // 📌 Hàm đăng xuất
  logout() {
    localStorage.removeItem('user'); // Xóa token khỏi LocalStorage
    this.currentUser.set(null);      // Cập nhật trạng thái đăng xuất
  }
}

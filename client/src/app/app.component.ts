import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavComponent } from "./nav/nav.component";
import { AccountService } from './_services/account.service';
import { HomeComponent } from "./home/home.component";
import { NgxSpinnerComponent } from 'ngx-spinner';

@Component({
  selector: 'app-root',
  standalone : true,
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
  imports: [RouterOutlet, NavComponent, NgxSpinnerComponent] // Nhúng NavComponent vào app chính
 // Nhúng NavComponent vào app chính
})
export class AppComponent implements OnInit {
  private accountService = inject(AccountService);

  // 📌 Khi ứng dụng khởi động, lấy danh sách người dùng
  ngOnInit(): void {
    this.setCurrentUser();
  }

  // 📌 Lấy dữ liệu user từ LocalStorage khi tải lại trang
  setCurrentUser() {
    const userString = localStorage.getItem('user');
    if(!userString) return;
    const user  = JSON.parse(userString);
    this.accountService.currentUser.set(user);
  }


}

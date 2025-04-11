import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { TitleCasePipe } from '@angular/common';
import { HasRoleDirective } from '../_directives/has-role.directive';

@Component({
  selector: 'app-nav',
  standalone : true,
  imports: [FormsModule, BsDropdownModule, RouterLink, RouterLinkActive, TitleCasePipe, HasRoleDirective],
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css'
})
export class NavComponent {
  accountService = inject(AccountService);  // Inject AccountService
  private router = inject(Router)
  private toastr = inject(ToastrService)
  model: any = {}; // Dữ liệu nhập từ form đăng nhập

  // 📌 Hàm đăng nhập
  login() {
    this.accountService.login(this.model).subscribe({
      next: _ => {
        void this.router.navigateByUrl('/members')
        this.toastr.show("You were Signin")
      },
      error : error => this.toastr.error(error.error)
    });
  }

  // 📌 Hàm đăng xuất
  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
    this.toastr.show("You were Logout")

  }
}

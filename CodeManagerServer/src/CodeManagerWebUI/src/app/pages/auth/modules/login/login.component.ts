import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { HttpClient } from '@angular/common/http';
import { ConfigService } from '../../../../config/config.service';
import { InputComponent } from '../../../../shared/input/input.component';
import { LoginResponseDto } from '../../login.interface';
import { UserDto } from '../../../user/user.interface';
import { AuthService } from '../../auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
  @ViewChild('username') usernameRef?: InputComponent;
  @ViewChild('password') passwordRef?: InputComponent;

  constructor(
    private readonly router: Router,
    private readonly toastrService: ToastrService,
    private readonly loginService: AuthService,
  ) {}

  ngOnInit(): void {}

  async navigateToRegistration() {
    await this.router.navigate(['register']);
  }

  async login() {
    // TODO: use login service to login
    await this.loginService.login(
      this.usernameRef?.value ?? '',
      this.passwordRef?.value ?? '',
    );
    this.router.navigate(['teams']);
  }
}

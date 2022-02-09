import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { InputComponent } from '../../../../shared/input/input.component';
import { AuthService } from '../../../../services/auth/auth.service';

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
    private readonly loginService: AuthService,
  ) {}

  ngOnInit(): void {}

  async navigateToRegistration() {
    await this.router.navigate(['register']);
  }

  async login() {
    await this.loginService.login(
      this.usernameRef?.value ?? '',
      this.passwordRef?.value ?? '',
    );
    this.router.navigate(['teams']);
  }
}

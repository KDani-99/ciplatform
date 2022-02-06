import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { ConfigService } from '../../../../config/config.service';
import { PasswordMismatchError } from '../login/errors/password-mismatch.error';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent implements OnInit {
  constructor(
    private readonly router: Router,
    private readonly httpClient: HttpClient,
    private readonly configService: ConfigService,
    private readonly toastrService: ToastrService,
  ) {}

  ngOnInit(): void {}

  async navigateToLogin() {
    await this.router.navigate(['login']);
  }

  async register(
    username: string,
    name: string,
    email: string,
    password: string,
    passwordAgain: string,
  ) {
    if (password !== passwordAgain) {
      this.toastrService.error('Different passwords provided!');
      return;
    }
    console.log(this.configService.getFullUrl('register'));
    const response = await this.httpClient
      .post(this.configService.getFullUrl('register'), {
        username,
        name,
        email,
        password,
      })
      .toPromise();

    console.log(response);
  }
}

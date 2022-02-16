import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { ConfigService } from '../../../../config/config.service';
import { firstValueFrom } from 'rxjs';

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
      return;
    }

    await firstValueFrom(
      this.httpClient.post(this.configService.getFullUrl('register'), {
        username,
        name,
        email,
        password,
      }),
    );

    this.router.navigate(['login']);
  }
}

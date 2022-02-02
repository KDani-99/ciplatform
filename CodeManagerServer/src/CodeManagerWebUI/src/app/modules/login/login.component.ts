import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { HttpClient } from '@angular/common/http';
import { ConfigService } from '../../config/config.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
  constructor(
    private readonly router: Router,
    private readonly toastrService: ToastrService,
    private readonly configService: ConfigService,
    private readonly httpClient: HttpClient,
  ) {}

  ngOnInit(): void {}

  async navigateToRegistration() {
    await this.router.navigate(['register']);
  }

  async login(username: string, password: string) {
    const response = await this.httpClient
      .post(this.configService.getFullUrl('login'), { username, password })
      .toPromise();
    console.log(response);
  }
}

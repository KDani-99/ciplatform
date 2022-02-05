import { Injectable } from '@angular/core';
import { ConfigService } from '../config/config.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { LoginResponseDto } from './login.interface';
import { UserDto } from '../user/user.interface';
import { Router } from '@angular/router';
import { Select, Store } from '@ngxs/store';
import { SetUser } from '../store/app/app.actions';
import { AppState, AppStateData } from '../store/app/app.state';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: '',
    }),
  };

  appState$?: Observable<AppStateData>;

  constructor(
    private readonly router: Router,
    private readonly configService: ConfigService,
    private readonly httpClient: HttpClient,
    private readonly store: Store,
  ) {
    this.appState$ = store.select((state) => state.app);
  }

  async logout(): Promise<void> {
    this.appState$?.subscribe({
      next: async (data: AppStateData) => {
        this.httpOptions.headers = this.httpOptions.headers.set(
          'Authorization',
          `Bearer ${data.user?.refreshToken}`,
        );
        await this.httpClient
          .post(this.configService.getFullUrl('logout'), this.httpOptions)
          .toPromise();

        this.store.dispatch(new SetUser(undefined));
        this.router.navigate(['login']);
      },
    });
  }

  async login(username: string, password: string): Promise<void> {
    this.store.dispatch(new SetUser(undefined)); // remove user

    const loginResponse = await this.httpClient
      .post<LoginResponseDto>(this.configService.getFullUrl('login'), {
        username,
        password,
      })
      .toPromise();

    this.httpOptions.headers = this.httpOptions.headers.set(
      'Authorization',
      `Bearer ${loginResponse.accessToken}`,
    );

    const user = await this.httpClient
      .get<UserDto>(
        this.configService.getFullUrl('getUser'),
        this.httpOptions, // TODO: add access token
      )
      .toPromise();

    this.store.dispatch([
      new SetUser({
        accessToken: loginResponse.accessToken,
        refreshToken: loginResponse.refreshToken,
        user,
      }),
    ]);
  }
}

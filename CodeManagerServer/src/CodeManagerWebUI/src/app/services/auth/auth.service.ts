import { Injectable } from '@angular/core';
import { ConfigService } from '../../config/config.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { LoginResponseDto } from './login.interface';
import { UserDto } from '../user/user.interface';
import { Router } from '@angular/router';
import { Store } from '@ngxs/store';
import { SetAuthTokens, SetUser } from '../../state/app/app.actions';
import { AppStateData } from '../../state/app/app.state';
import { Observable, firstValueFrom } from 'rxjs';

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

  async logout(): Promise<any> {
    const appState = await firstValueFrom(this.appState$!);

    this.httpOptions.headers = this.httpOptions.headers.set(
      'Authorization',
      `Bearer ${appState.user?.refreshToken}`,
    );

    await firstValueFrom(
      this.httpClient.post(
        this.configService.getFullUrl('logout'),
        {},
        this.httpOptions,
      ),
    );

    await this.store.dispatch(new SetUser(undefined));
    this.router.navigate(['login']);
  }

  async login(username: string, password: string): Promise<void> {
    await firstValueFrom(this.store.dispatch(new SetUser(undefined))); // remove user

    const loginResponse = await firstValueFrom(
      this.httpClient.post<LoginResponseDto>(
        this.configService.getFullUrl('login'),
        {
          username,
          password,
        },
      ),
    );

    this.httpOptions.headers = this.httpOptions.headers.set(
      'Authorization',
      `Bearer ${loginResponse.accessToken}`,
    );

    const user = await firstValueFrom(
      this.httpClient.get<UserDto>(
        this.configService.getFullUrl('getUser'),
        this.httpOptions,
      ),
    );

    this.store.dispatch([
      new SetUser({
        accessToken: loginResponse.accessToken,
        refreshToken: loginResponse.refreshToken,
        user,
      }),
    ]);
  }

  async refreshToken(): Promise<string> {
    const refreshToken = this.store.selectSnapshot<string | undefined>(
      (state) => state.app.user.refreshToken,
    );
    this.httpOptions.headers = this.httpOptions.headers.set(
      'Authorization',
      `Bearer ${refreshToken}`,
    );

    try {
      const response = await firstValueFrom(
        this.httpClient.post<LoginResponseDto>(
          this.configService.getFullUrl('refreshToken'),
          {},
          this.httpOptions,
        ),
      );

      await firstValueFrom(
        this.store.dispatch(
          new SetAuthTokens(response.accessToken, response.refreshToken),
        ),
      );

      return response.accessToken;
    } catch (error: any) {
      this.store
        .dispatch(new SetUser(undefined))
        .subscribe(() => this.router.navigate(['login']));
      throw error;
    }
  }
}

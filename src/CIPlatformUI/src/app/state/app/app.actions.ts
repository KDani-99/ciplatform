import { LoggedUser } from './app.state';

export class SetUser {
  static readonly type = '[app] set user';
  constructor(public user?: LoggedUser) {}
}
export class SetAuthTokens {
  static readonly type = '[app] set auth tokens';
  constructor(public accessToken: string, public refreshToken: string) {}
}

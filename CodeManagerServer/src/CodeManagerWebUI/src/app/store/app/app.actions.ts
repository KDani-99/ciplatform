import { LoggedUser } from './app.state';

export class SetUser {
  static readonly type = '[app] set user';
  constructor(public user?: LoggedUser) {}
}

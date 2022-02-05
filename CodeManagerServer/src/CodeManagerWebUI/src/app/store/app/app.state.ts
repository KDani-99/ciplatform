import { Action, Selector, State, StateContext } from '@ngxs/store';
import { SetUser } from './app.actions';
import { UserDto } from '../../user/user.interface';
import { Injectable } from '@angular/core';

export interface LoggedUser {
  user: UserDto;
  accessToken: string;
  refreshToken: string;
}

export interface AppStateData {
  user?: LoggedUser;
}

@State<AppStateData>({
  name: 'app',
  defaults: {
    user: undefined,
  },
})
@Injectable()
export class AppState {
  @Action(SetUser)
  setUser({ patchState }: StateContext<AppStateData>, { user }: SetUser) {
    patchState({ user });
  }
  @Selector()
  static isAdmin(state: AppStateData) {
    return state.user?.user.isAdmin ?? false;
  }
  @Selector()
  static isLoggedIn(state: AppStateData) {
    return typeof state.user !== 'undefined';
  }
}

import { Action, Selector, State, StateContext } from '@ngxs/store';
import { SetAuthTokens, SetUser } from './app.actions';
import { UserDto } from '../../pages/user/user.interface';
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
  @Action(SetAuthTokens)
  setAuthTokens(
    ctx: StateContext<AppStateData>,
    { accessToken, refreshToken }: SetAuthTokens,
  ) {
    ctx.patchState({
      user: { ...ctx.getState().user, accessToken, refreshToken } as any,
    });
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

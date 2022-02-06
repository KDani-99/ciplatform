import {
  ActivatedRouteSnapshot,
  CanActivate,
  Router,
  RouterStateSnapshot,
  UrlTree,
} from '@angular/router';
import { Injectable } from '@angular/core';
import { Select } from '@ngxs/store';
import { AppState } from '../store/app/app.state';
import { Observable } from 'rxjs';

@Injectable()
export class AuthenticatedGuard implements CanActivate {
  @Select(AppState.isLoggedIn) isLoggedIn$?: Observable<boolean>;

  constructor(private router: Router) {}

  async canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot,
  ): Promise<boolean | UrlTree> {
    this.isLoggedIn$?.subscribe({
      next: (value) => {
        if (!value) {
          this.router.navigate(['login']);
        }
      },
    });
    return true;
  }
}

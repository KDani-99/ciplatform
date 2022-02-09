import { Component, OnInit } from '@angular/core';
import { Select, Store } from '@ngxs/store';
import { UserDto } from '../../../../services/user/user.interface';
import { Observable } from 'rxjs';
import { AuthService } from '../../../../services/auth/auth.service';

@Component({
  selector: 'nav-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss'],
})
export class UserComponent implements OnInit {
  user$?: Observable<UserDto>;

  constructor(
    private readonly store: Store,
    private readonly loginService: AuthService,
  ) {
    this.user$ = store.select((state) => state.app.user.user);
  }

  ngOnInit(): void {}

  logout(): Promise<any> {
    return this.loginService.logout();
  }
}

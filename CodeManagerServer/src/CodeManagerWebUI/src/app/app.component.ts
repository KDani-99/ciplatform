import { Component } from '@angular/core';
import { Select } from '@ngxs/store';
import { AppState } from './store/app/app.state';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  title = 'CodeManagerWebUI';

  @Select(AppState.isLoggedIn) isLoggedIn$?: Observable<boolean>;
}

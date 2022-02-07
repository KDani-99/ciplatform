import { Component, OnInit } from '@angular/core';
import { Select } from '@ngxs/store';
import { AppState } from './state/app/app.state';
import { Observable } from 'rxjs';
import { SignalRService } from './services/signalr/signalr.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  title = 'CodeManagerWebUI';

  @Select(AppState.isLoggedIn) isLoggedIn$?: Observable<boolean>;

  constructor(private readonly signalrService: SignalRService) {}

  ngOnInit(): void {
    this.signalrService.connect();
  }
}

import {
  Component,
  ComponentRef,
  ElementRef,
  OnInit,
  ViewChild,
} from '@angular/core';
import { Select } from '@ngxs/store';
import { AppState } from './state/app/app.state';
import { Observable } from 'rxjs';
import { SignalRService } from './services/signalr/signalr.service';
import { ErrorService } from './services/error/error.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  title = 'CodeManagerWebUI';

  @Select(AppState.isLoggedIn) isLoggedIn$?: Observable<boolean>;

  constructor(
    private readonly signalrService: SignalRService,
    private readonly errorService: ErrorService,
  ) {}

  ngOnInit(): void {
    this.signalrService.connect();
  }

  closeError(): void {
    this.errorService.hideError();
  }

  showError(): Observable<boolean> {
    return this.errorService.displayError();
  }

  getErrorMessage(): Observable<Record<string, string[]> | string> {
    return this.errorService.errorMessage;
  }
}

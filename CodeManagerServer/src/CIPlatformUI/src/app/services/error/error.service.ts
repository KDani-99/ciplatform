import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ErrorService {
  private error!: Record<string, string[]> | string;
  private showError!: boolean;

  constructor() {
    this.showError = false;
  }

  show(error: Record<string, string[]> | string): void {
    this.showError = true;
    this.error = error;
  }

  hideError() {
    this.showError = false;
  }

  displayError(): Observable<boolean> {
    return new Observable<boolean>((subscriber) =>
      subscriber.next(this.showError),
    );
  }

  get errorMessage(): Observable<Record<string, string[]> | string> {
    return new Observable<Record<string, string[]> | string>((subscriber) =>
      subscriber.next(this.error),
    );
  }
}

import { Injectable } from '@angular/core';
import * as signalr from '@microsoft/signalr';
import { ConfigService } from '../../config/config.service';
import { firstValueFrom, Observable } from 'rxjs';
import { Select, Store } from '@ngxs/store';
import { AppState } from '../../state/app/app.state';

@Injectable({
  providedIn: 'root',
})
export class SignalRService {
  private hubConnection: signalr.HubConnection | undefined;

  @Select((state: any) => state.app.user.accessToken)
  accessToken!: Observable<string>;

  constructor(private readonly configService: ConfigService) {}

  connect(): Promise<any> {
    const ctx = this;
    this.hubConnection = new signalr.HubConnectionBuilder()
      .withUrl(this.configService.getWsAddress('runs')!, {
        async accessTokenFactory(): Promise<string> {
          // we can't access the this context here
          return firstValueFrom(ctx.accessToken);
        },
      })
      .build();

    this.registerMethods();

    return this.hubConnection.start();
  }

  private registerMethods() {
    this.hubConnection?.on('ReceiveLogs', this.receiveLogs);
  }

  subscribeToChannel(runId: number, jobId: number) {
    this.hubConnection?.send('SubscribeToChannel', runId, jobId);
  }

  receiveLogs(logData: any) {
    console.log('RECEIVED LOG: ', logData);
  }

  registerMethod(name: string, action: any) {
    this.hubConnection?.off(name);
    this.hubConnection?.on(name, action);
  }

  disconnect() {
    this.hubConnection?.stop();
  }
}

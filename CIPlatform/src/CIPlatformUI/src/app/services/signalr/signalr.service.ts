import { Injectable } from '@angular/core';
import * as signalr from '@microsoft/signalr';
import { ConfigService } from '../../config/config.service';
import { firstValueFrom, Observable } from 'rxjs';
import { Select, Store } from '@ngxs/store';
import { AppState } from '../../state/app/app.state';

export enum ResultsChannel {
  PROJECT = 'project',
  RUN = 'run',
  JOB = 'job',
  STEP = 'step',
}

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
          // we can't access the 'this' context here
          return firstValueFrom(ctx.accessToken);
        },
      })
      .withAutomaticReconnect()
      .build();


    return this.hubConnection.start();
  }

  subscribeToResultsChannel(resultsChannel: ResultsChannel, entityId: number) {
    this.hubConnection?.send(
      'SubscribeToResultsChannel',
      resultsChannel,
      entityId,
    );
  }

  unSubscribeFromResultsChannel(
    resultsChannel: ResultsChannel,
    entityId: number,
  ) {
    this.hubConnection?.send(
      'UnSubscribeFromChannel',
      resultsChannel,
      entityId,
    );
  }

  registerMethod(name: string, action: any) {
    this.hubConnection?.off(name);
    this.hubConnection?.on(name, action);
  }

  send(name: string, ...args: any[]): Promise<void> {
    return this.hubConnection!.send(name, args);
  }

  disconnect() {
    this.hubConnection?.stop();
  }
}

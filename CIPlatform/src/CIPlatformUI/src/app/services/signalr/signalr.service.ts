import {Injectable} from '@angular/core';
import * as signalr from '@microsoft/signalr';
import {ConfigService} from '../../config/config.service';
import {firstValueFrom, Observable} from 'rxjs';
import {Select} from '@ngxs/store';
import {HubConnectionState} from "@microsoft/signalr/dist/esm/HubConnection";

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

  constructor(private readonly configService: ConfigService) {
    this.connect().then(() => console.log('Connected'));
  }

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

  async subscribeToResultsChannel(resultsChannel: ResultsChannel, entityId: number): Promise<void> {
    while (this.hubConnection?.state === HubConnectionState.Connecting) {
      await this.delayTask(2.5 * 1000)
    }
    this.hubConnection?.send(
      'SubscribeToResultsChannel',
      resultsChannel,
      entityId,
    );
  }

  async unSubscribeFromResultsChannel(
    resultsChannel: ResultsChannel,
    entityId: number,
  ): Promise<void> {
    while (this.hubConnection?.state === HubConnectionState.Connecting) {
      await this.delayTask(2.5 * 1000)
    }
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

  async send(name: string, ...args: any[]): Promise<void> {
    while (this.hubConnection?.state === HubConnectionState.Connecting) {
      await this.delayTask(2.5 * 1000)
    }
    return this.hubConnection!.send(name, args);
  }

  private delayTask(time: number): Promise<void> {
    return new Promise((resolve) => {
      setTimeout(() => resolve(), time);
    });
  }
}

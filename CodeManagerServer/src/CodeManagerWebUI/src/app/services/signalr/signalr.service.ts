import { Injectable } from '@angular/core';
import * as signalr from '@microsoft/signalr';
import { ConfigService } from '../../config/config.service';

@Injectable({
  providedIn: 'root',
})
export class SignalRService {
  private hubConnection: signalr.HubConnection | undefined;

  constructor(private readonly configService: ConfigService) {}

  connect() {
    this.hubConnection = new signalr.HubConnectionBuilder()
      .withUrl(this.configService.getWsAddress()!)
      .build();
    this.hubConnection
      .start()
      .then(() => console.log('Connecting to hub...'))
      .catch((err: any) => console.log('Error: ' + err));
  }
}

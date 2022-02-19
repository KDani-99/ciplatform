import { Injectable } from '@angular/core';
import { Config } from './config.interface';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ConfigService {
  public appConfig: Config | null = null;

  constructor(private readonly httpClient: HttpClient) {}

  async loadConfig() {
    this.appConfig = (await firstValueFrom(
      this.httpClient.get('/assets/config/config.json'),
    )) as Config;
  }

  getFullUrl(key: string): string {
    return `${this.appConfig?.api.baseUrl}/${this.appConfig?.api.endpoints[key]?.url}`;
  }

  getWsAddress(key: string): string | undefined {
    return this.appConfig?.hubs[key];
  }
}

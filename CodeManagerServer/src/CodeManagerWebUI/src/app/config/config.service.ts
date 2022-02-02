import { Injectable } from '@angular/core';
import { Config } from './config.interface';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class ConfigService {
  public appConfig: Config | null = null;

  constructor(private readonly httpClient: HttpClient) {}

  async loadConfig() {
    this.appConfig = (await this.httpClient
      .get('/assets/config/config.json')
      .toPromise()) as Config;
  }

  getFullUrl(key: string) {
    return `${this.appConfig?.api.baseUrl}/${this.appConfig?.api.endpoints[key]}`;
  }
}

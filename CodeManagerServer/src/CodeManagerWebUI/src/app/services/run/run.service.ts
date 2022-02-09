import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ConfigService } from '../../config/config.service';
import { BaseDataService } from '../../shared/services/base-data.service';
import { RunDataDto } from './run.interface';

@Injectable({ providedIn: 'root' })
export class RunService extends BaseDataService {
  protected formHttpOptions = {
    withCredentials: true,
  };
  constructor(
    httpClient: HttpClient,
    private readonly configService: ConfigService,
  ) {
    super(httpClient);
  }

  startRun(id: number, instructionsFile: File): Observable<any> {
    const url = this.configService
      .getFullUrl('uploadRunInstructions')
      .replace('{0}', id.toString());
    const formData = new FormData();
    formData.append('instructions', instructionsFile, instructionsFile.name);
    return this.httpClient.post(url, formData, this.formHttpOptions);
  }

  getRun(id: number): Observable<RunDataDto> {
    const url = `${this.configService.getFullUrl('runs')}/${id}`;
    return super.get<RunDataDto>(url);
  }
}

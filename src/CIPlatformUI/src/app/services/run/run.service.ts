import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ConfigService } from '../../config/config.service';
import { BaseDataService } from '../../shared/services/base-data.service';
import {JobDataDto, JobDto, RunDataDto, RunDto, State} from './run.interface';

@Injectable({ providedIn: 'root' })
export class RunService extends BaseDataService {
  protected formHttpOptions = {
    withCredentials: true,
  };
  protected textHttpOptions = {
    withCredentials: true,
    headers: new HttpHeaders({ 'Content-Type': 'text/plain' }),
  };
  constructor(
    httpClient: HttpClient,
    private readonly configService: ConfigService,
  ) {
    super(httpClient);
  }

  startRun(id: number, instructionsFile: File): Observable<RunDto> {
    const url = this.configService
      .getFullUrl('uploadRunInstructions')
      .replace('{0}', id.toString());
    const formData = new FormData();
    formData.append('instructions', instructionsFile, instructionsFile.name);
    return this.httpClient.post<RunDto>(url, formData, this.formHttpOptions);
  }

  getRun(id: number): Observable<RunDataDto> {
    const url = `${this.configService.getFullUrl('runs')}/${id}/data`;
    return super.get<RunDataDto>(url);
  }

  getJob(runId: number, jobId: number): Observable<JobDataDto> {
    const url = `${this.configService.getFullUrl('runs')}/${runId}/${jobId}`;
    return this.httpClient.get<JobDataDto>(url, this.httpOptions);
  }

  getStepFile(runId: number, jobId: number, stepId: number): Observable<any> {
    const url = `${this.configService.getFullUrl(
      'runs',
    )}/${runId}/${jobId}/${stepId}`;
    return this.httpClient.get(url, {
      responseType: 'text',
      withCredentials: true,
    });
  }

  deleteRun(runId: number): Observable<any> {
    const url = `${this.configService.getFullUrl('runs')}/${runId}`;
    return this.httpClient.delete(url, this.httpOptions);
  }

  getTime(time: string): string {
    return !time ? '' : new Date(time).toLocaleString();
  }

  getFormattedExecutionTime(
    startDateTime: string,
    finishedDateTime: string,
  ): string {
    let value = 0;
    if (startDateTime && finishedDateTime) {
      value =
        new Date(finishedDateTime).getTime() -
        new Date(startDateTime).getTime();
    }
    return new Date(value).toISOString().substr(11, 8);
  }
}

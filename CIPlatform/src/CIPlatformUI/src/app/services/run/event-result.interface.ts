import { State } from './run.interface';

export interface ProcessedStepResult {
  jobId: number;
  stepId: number;
  dateTime: string;
  state: State;
}

export interface ProcessedJobResult {
  runId: number;
  jobId: number;
  dateTime: string;
  state: State;
}

export interface ProcessedRunResult {
  projectId: number;
  runId: number;
  dateTime: string;
  state: State;
}

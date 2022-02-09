export enum State {
  NOT_RUN,
  SUCCESSFUL,
  FAILED,
  CANCELLED,
  SKIPPED,
  QUEUED,
  RUNNING,
}
export interface JobDto {
  id: number;
  startedDateTime: string;
  finishedDateTime: string;
  executionTime: number;
  jobContext: string;
  steps: number;
  state: State;
}
export interface RunDto {
  id: number;
  startedDateTime: string;
  finishedDateTime: string;
  executionTime: number;
  state: State;
  jobs: number;
}
export interface RunDataDto {
  run: RunDto;
  jobs: JobDto[];
}

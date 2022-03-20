export enum State {
  NOT_RUN,
  SUCCESSFUL,
  FAILED,
  SKIPPED,
  QUEUED,
  RUNNING,
}
export interface StepDto {
  id: number;
  name: string;
  startedDateTime: string;
  finishedDateTime: string;
  executionTime: number;
  state: State;
}
export interface JobDto {
  id: number;
  startedDateTime: string;
  name: string;
  finishedDateTime: string;
  executionTime: number;
  jobContext: string;
  steps: number;
  state: State;
}
export interface JobDataDto {
  job: JobDto;
  steps: StepDto[];
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

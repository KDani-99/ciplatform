import { MemberPermission } from '../team/team.interface';
import { RunDto } from '../run/run.interface';

export interface ProjectDto {
  id: number;
  name: string;
  description: string;
  owner: string;
  isPrivateProject: boolean;
  teamName: string;
  runs: number;
}
export interface ProjectDataDto {
  project: ProjectDto;
  repositoryUrl: string;
  teamId: number;
  userPermission: MemberPermission;
  runs: RunDto[];
}
export interface CreateProjectDto {
  teamId: number;
  name: string;
  description: string;
  repositoryUrl: string;
  isPrivateProject: boolean;
}

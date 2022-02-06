import { MemberPermission } from '../team/team.interface';

export interface VariableDto {
  name: string;
  value: string;
  isSecret: boolean;
}
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
  isPrivateRepository: boolean;
  repositoryUrl: string;
  teamId: number;
  userPermission: MemberPermission;
  variables: VariableDto[];
}
export interface CreateProjectDto {
  teamId: number;
  name: string;
  description: string;
  repositoryUrl: string;
  isPrivateRepository: boolean;
  isPrivateProject: boolean;
  username?: string;
  secretToken?: string;
}

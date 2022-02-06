import { ProjectDto } from '../project/project.interface';
export enum MemberPermission {
  READ = 0,
  READ_WRITE = 1,
  ADMIN = 2,
}
export interface MemberDto {
  id: number;
  name: string;
  username: string;
  image: string;
  joinTime: string;
  permission: MemberPermission;
}
export interface TeamDto {
  id: number;
  isPublic: boolean;
  isMember: boolean;
  name: string;
  description: string;
  image: string;
  members: number;
  projects: number;
  owner: string;
}
export interface TeamDataDto {
  id: number;
  isPublic: boolean;
  name: string;
  description: string;
  image: string;
  members: MemberDto[];
  projects: ProjectDto[];
  owner: string;
  userPermission: MemberPermission;
}
export interface CreateTeamDto {
  name: string;
  description: string;
  image: string;
  isPublic: boolean;
}

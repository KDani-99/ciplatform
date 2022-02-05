export interface ProjectDto {
  id: number;
  name: string;
  description: string;
  isPrivateRepository: boolean;
  isPrivateProject: boolean;
  teamName: string;
  runs: number;
  owner: string;
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

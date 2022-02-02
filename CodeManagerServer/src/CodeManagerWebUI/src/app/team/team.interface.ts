export interface TeamDto {
  id: number;
  isPublic: boolean;
  name: string;
  description: string;
  image: string;
  members: number;
  projects: number;
  owner: string;
}
export interface CreateTeamDto {}

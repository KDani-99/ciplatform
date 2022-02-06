export interface MemberDto {
  id: number;
  name: string;
  username: string;
  image: string;
  joinTime: string;
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
export interface CreateTeamDto {
  name: string;
  description: string;
  image: string;
  isPublic: boolean;
}

interface TeamMember {
  id: number;
  username: string;
  image: string;
}
export interface Team {
  id: number;
  isPublic: boolean;
  name: string;
  description: string;
  image: string;
  members: number;
  projects: number;
}

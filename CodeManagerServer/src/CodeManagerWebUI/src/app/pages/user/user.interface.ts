export interface UpdateUserDto {
  username: string;
  name: string;
  email: string;
  imageUrl: string;
  isAdmin: boolean;
}
export interface UserDto {
  id: number;
  teams: number;
  username: string;
  email: string;
  name: string;
  isAdmin: boolean;
  plan: string;
  registration: string;
  image: string;
}

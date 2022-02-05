export interface LoginDto {
  username: string;
  password: string;
}
export interface LoginResponseDto {
  accessToken: string;
  refreshToken: string;
}

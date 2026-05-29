export interface LoginDto {
  email: string;
  password: string;
}

export interface AccessTokenVm {
  accessToken: string;
}

export interface JwtUser {
  id: string;
  email: string;
  perm: number;
  roles: string[];
}

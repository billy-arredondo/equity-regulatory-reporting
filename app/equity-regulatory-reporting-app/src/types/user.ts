export interface UserDto {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  roles: string[];
}

export interface UpdateUserDto {
  firstName: string;
  lastName: string;
}

export interface AssignRoleDto {
  userId: string;
  role: string;
}

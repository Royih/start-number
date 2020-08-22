import React, { useEffect, useState, useContext } from "react";
import { ApiContext } from "./ApiContextProvider";

export type ClaimDto = {
  type: string;
  value: string;
};

export type UserDto = {
  id: string;
  name: string;
  tenantId: string;
  tenantName: string;
  roles: [];
  claims: ClaimDto[];
};

export enum RoleTypes {
  Admin = "Admin",
  User = "User",
}

export type UserContextState = {
  user: UserDto;
  debugMode: boolean;
  logout: () => void;
  hasRole: (role: RoleTypes) => boolean;
  hasAnyRoles: (role: RoleTypes[]) => boolean;
  toggleDebugMode: () => void;
};

const UserContext = React.createContext({} as UserContextState); // Create a context object

export { UserContext };

export const UserContextProvider = (props: any) => {
  const [user, setUser] = useState<UserDto>();
  const [debugMode, setDebugMode] = useState(false);
  const api = useContext(ApiContext);

  useEffect(() => {
    var userFromExistingToken = api.getUserFromExistingToken();
    if (userFromExistingToken) {
      setUser(userFromExistingToken);
    }
  }, [api]);

  const logout = () => {
    api.logout();
    setUser(undefined);
  };

  // const login = (loginResult: ILoginResult) => {
  //   var user = api.login(loginResult);
  //   setUser(user);
  // };

  return (
    <UserContext.Provider
      value={{
        user: user as UserDto,
        logout: logout,
        debugMode: debugMode,
        hasRole: (role: RoleTypes) => {
          return user ? user.claims.filter((x) => x.type === "role" && x.value === role.toString()).length > 0 : false;
        },
        hasAnyRoles: (roles: RoleTypes[]) => {
          return user ? user.claims.filter((x) => x.type === "role" && roles.filter((y) => y.toString() === x.value).length > 0).length > 0 : false;
        },
        toggleDebugMode: () => {
          setDebugMode((prevValue: boolean) => !prevValue);
        },
      }}
    >
      {props.children}
    </UserContext.Provider>
  );

  // if (user) {
  //     return (
  //         <UserContext.Provider
  //             value={{
  //                 user: user as UserDto,
  //                 logout: logout,
  //                 debugMode: debugMode,
  //                 hasRole: (role: RoleTypes) => {
  //                     return user.claims.filter(x => x.type === "role" && x.value === role.toString()).length > 0;
  //                 },
  //                 hasAnyRoles: (roles: RoleTypes[]) => {
  //                     return user.claims.filter(x => x.type === "role" && roles.filter(y => y.toString() === x.value).length > 0).length > 0;
  //                 },
  //                 toggleDebugMode: () => {
  //                     setDebugMode((prevValue: boolean) => !prevValue);
  //                 }
  //             }}
  //         >
  //             {props.children}
  //         </UserContext.Provider>
  //     );
  // }
  // return <Login login={login} />;
};

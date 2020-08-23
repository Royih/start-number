import React, { useEffect, useState, useContext } from "react";
import { ActiveEventDto } from "src/models";
import { ApiContext } from "src/infrastructure/ApiContextProvider";
import { useHistory } from "react-router-dom";
import { makeStyles, Table, TableContainer, TableHead, TableRow, TableCell, TableBody, IconButton } from "@material-ui/core";
import { UserContext, RoleTypes } from "src/infrastructure/UserContextProvider";
import ViewEventIcon from "@material-ui/icons/ViewList";
import AdminEventIcon from "@material-ui/icons/Settings";

const useStyles = makeStyles((theme) => ({
  defaultCursor: {
    cursor: "default",
  },
}));

export const Home = () => {
  const api = useContext(ApiContext);
  const currentUser = useContext(UserContext);
  const [events, setEvents] = useState([] as ActiveEventDto[]);
  const history = useHistory();
  const classes = useStyles();

  useEffect(() => {
    const loadData = async () => {
      const data = await api.get<ActiveEventDto[]>("anonymous/listEvents");
      setEvents(data);
    };
    loadData();
  }, [api]);

  return (
    <div>
      <h4>
        <code>Select your event below</code>
      </h4>
      <TableContainer>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Event</TableCell>
              {currentUser && <TableCell></TableCell>}
            </TableRow>
          </TableHead>
          <TableBody>
            {events.map((ev: ActiveEventDto) => (
              <TableRow key={ev.tenantKey} hover className={classes.defaultCursor}>
                <TableCell onClick={() => history.push("/signup/" + ev.tenantKey)}>
                  {ev.logo && <img alt="Logo" height="100px" src={ev.logo} color="secondary" />}
                  {ev.name}
                </TableCell>
                {currentUser && (
                  <TableCell>
                    {currentUser.hasRole(RoleTypes.User) && (
                      <IconButton
                        onClick={() => {
                          history.push("/view/" + ev.eventId);
                        }}
                      >
                        <ViewEventIcon />
                      </IconButton>
                    )}
                    {currentUser.hasRole(RoleTypes.Admin) && (
                      <IconButton onClick={() => {}}>
                        <AdminEventIcon />
                      </IconButton>
                    )}
                  </TableCell>
                )}
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </div>
  );
};

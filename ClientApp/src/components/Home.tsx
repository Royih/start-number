import React, { useEffect, useState, useContext } from "react";
import { ActiveEventDto } from "src/models";
import { ApiContext } from "src/infrastructure/ApiContextProvider";
import { useHistory } from "react-router-dom";
import { makeStyles } from "@material-ui/core";

const useStyles = makeStyles((theme) => ({
  defaultCursor: {
    cursor: "default",
  },
}));

export const Home = () => {
  const api = useContext(ApiContext);
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
      <h1>Welcome to Signup!</h1>
      <p>Solving your need for sign-up and generating start-numbers for sport-events or similar.</p>
      
      <h2>Click your event below
        <ul>
          {events.map((ev: ActiveEventDto) => (
            <li
              className={classes.defaultCursor}
              key={ev.tenantKey}
              onClick={() => {
                history.push("/" + ev.tenantKey);
              }}
            >
              {ev.name}
            </li>
          ))}
        </ul>
      </h2>
    </div>
  );
};

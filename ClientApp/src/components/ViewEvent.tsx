import React, { useEffect, useState, useContext } from "react";
import { SignUpsForEventDto } from "src/models";
import { ApiContext } from "src/infrastructure/ApiContextProvider";
import { useParams } from "react-router-dom";
import { Table, TableContainer, TableHead, TableRow, TableCell, TableBody } from "@material-ui/core";

export const ViewEvent = () => {
  const api = useContext(ApiContext);
  const [signups, setSignups] = useState([] as SignUpsForEventDto[]);
  const { eventId } = useParams();

  useEffect(() => {
    const loadData = async () => {
      setSignups(await api.get<SignUpsForEventDto[]>("signup/listSignups/" + eventId));
    };
    loadData();
  }, [api, eventId]);

  return (
    <div>
      <TableContainer>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>#</TableCell>
              <TableCell>First name</TableCell>
              <TableCell>Surname</TableCell>
              <TableCell>Email</TableCell>
              <TableCell>Allow contact</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {signups &&
              signups.map((signup: SignUpsForEventDto) => (
                <TableRow key={signup.startNumber} hover>
                  <TableCell>{signup.startNumber}</TableCell>
                  <TableCell>{signup.firstName}</TableCell>
                  <TableCell>{signup.surName}</TableCell>
                  <TableCell>{signup.email}</TableCell>
                  <TableCell>{signup.allowUsToContactPersonByEmail ? "Yes" : "No"}</TableCell>
                </TableRow>
              ))}
          </TableBody>
        </Table>
      </TableContainer>
    </div>
  );
};

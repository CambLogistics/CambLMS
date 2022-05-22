namespace camblms

open WebSharper

[<JavaScript>]
type PendingFee = {ID: int;Username: string;Amount: int}

module ServiceFee =
    let getPendingFees sid =
        if not <| User.verifyAdmin sid then []
        else
        try
            let db = Database.SqlConnection.GetDataContext (Database.getConnectionString())
            query{
                for fee in db.Camblogistics.ServiceFees do
                where (fee.Paid = (sbyte 0))
                join u in db.Camblogistics.Users on (fee.UserId = u.Id)
                select({ID = fee.Id;Username = u.Name;Amount = fee.Amount})
            } |> Seq.toList
        with
           _ -> []
    let submitPendingFee sid userid amount =
        if not <| User.verifyAdmin sid then ()
        else
        //try
            let db = Database.SqlConnection.GetDataContext (Database.getConnectionString())
            let newFee = db.Camblogistics.ServiceFees.Create()
            newFee.Amount <- amount
            newFee.UserId <- userid
            newFee.Date <- System.DateTime.Now
            newFee.Paid <- (sbyte 0)
            db.SubmitUpdates()
        (*with
            _ -> ()*)
    let payFee sid feeID =
        if not <| User.verifyAdmin sid then ()
        try
        let db = Database.SqlConnection.GetDataContext (Database.getConnectionString())
        (query{
            for f in db.Camblogistics.ServiceFees do
            where(f.Id = feeID)
            exactlyOne
        }).Paid <- (sbyte 1)
        db.SubmitUpdates()
        with
            _ -> ()
    [<Rpc>]
    let doPayFee sid feeID =
        async{
            payFee sid feeID
        }
    [<Rpc>]
    let doSubmitFee sid userID amount =
        async{
            submitPendingFee sid userID amount
        }
    [<Rpc>]
    let doGetPendingFees sid =
        async{
            return getPendingFees sid
        }
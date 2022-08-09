namespace camblms

module RandomString =
    let getRandomString length =
        let chars = "ABCDEFGHIJKLMNOPQRSTUVXYZabcdefghijklmnopqrstuvxyz0123456789"
        let rnd = System.Random()
        System.String(Array.init length (fun _ -> chars[rnd.Next chars.Length]))